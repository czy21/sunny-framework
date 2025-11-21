import axios, {AxiosRequestConfig} from 'axios'
import {ElMessage} from "element-plus";


enum Method {
    GET = "GET",
    POST = "POST",
    PUT = "PUT",
    DELETE = "DELETE"
}

const service = axios.create({
    baseURL: import.meta.env.VITE_BASE_API,
    withCredentials: true
});

service.interceptors.request.use(
    config => {
        return config;
    },
    error => {
        return Promise.reject(error)
    });
service.interceptors.response.use(
    response => {
        const {code, message} = response.data
        if (code === -1) {
            ElMessage({
                type: "error",
                message: message,
                duration: 3000,
            })
        }
        return response
    },
    error => {
        const {status} = error.response || {};
        switch (status) {
            case 401:
                ElMessage({
                    type: "error",
                    message: "登录信息过期，请重新登录",
                    duration: 3000,
                    onClose() {
                        localStorage.removeItem("isLoggedIn")
                        window.location.reload()
                    },
                })
                break
            case 500:
            case 502:
                ElMessage({
                    type: "error",
                    message: "服务器异常",
                    duration: 3000,
                })
                break
            case 504:
                ElMessage({
                    type: "error",
                    message: "网络超时",
                    duration: 3000,
                })
                break
        }
        return Promise.reject(error)
    });

function apiAxios(method: Method, url: string, params: any, config?: AxiosRequestConfig, errorCallBack?: (error: any) => void) {
    return new Promise((resolve, reject) => {
        service({
            method: method,
            url: url,
            data: method === 'POST' || method === 'PUT' ? params : null,
            params: method === 'GET' || method === 'DELETE' ? params : null,
            ...config
        })
            .then(res => resolve(res))
            .catch(error => {
                if (errorCallBack) {
                    errorCallBack(error)
                }
            })
    })
}

export const get = (url: string, params?: any, config?: AxiosRequestConfig, errorCallBack?: (error: any) => void) => {
    return apiAxios(Method.GET, url, params, config, errorCallBack)
}
export const post = (url: string, params?: any, config?: AxiosRequestConfig, errorCallBack?: (error: any) => void) => {
    return apiAxios(Method.POST, url, params, config, errorCallBack)
}
export const put = (url: string, params?: any, config?: AxiosRequestConfig, errorCallBack?: (error: any) => void) => {
    return apiAxios(Method.PUT, url, params, config, errorCallBack)
}
export const del = (url: string, params?: any, config?: AxiosRequestConfig, errorCallBack?: (error: any) => void) => {
    return apiAxios(Method.DELETE, url, params, config, errorCallBack)
}

export const checkVersion = () => {
    const versionKey = "version"
    let version = JSON.parse(localStorage.getItem(versionKey))
    get("version.json", {"date": new Date().getTime()}, {baseURL: "/"}).then((res: any) => {
        if (res.data?.buildDate != (version?.buildDate)) {
            localStorage.setItem(versionKey, JSON.stringify(res.data))
            window.location.reload()
        }
    })
}