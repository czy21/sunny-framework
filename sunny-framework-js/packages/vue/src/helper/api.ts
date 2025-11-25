import axios, {AxiosInstance, AxiosRequestConfig, AxiosResponse} from 'axios'
import {ElMessage} from "element-plus";

export enum Method {
    GET = "GET",
    POST = "POST",
    PUT = "PUT",
    DELETE = "DELETE"
}

export interface RequestOption {
    config?: AxiosRequestConfig;
    errorCallBack?: (error: any) => void;
}

export interface HttpClientOption {
    baseURL?: string;
    handleResponse?: (response: AxiosResponse<any>) => void;
}

export class HttpClient {

    private service: AxiosInstance;

    private readonly handleResponse: (response: AxiosResponse<any>) => void

    constructor({
                    baseURL = import.meta.env.VITE_BASE_API,
                    handleResponse = () => {
                    }
                }: HttpClientOption = {}) {
        this.service = axios.create({
            baseURL,
            withCredentials: true,
        });
        this.handleResponse = handleResponse
        this.setupInterceptors();
    }

    private setupInterceptors() {

        this.service.interceptors.request.use(
            (config) => config,
            (error) => Promise.reject(error)
        );

        this.service.interceptors.response.use(
            (response: AxiosResponse<any>) => {
                const {code, message} = response.data;
                if (code === -1) {
                    ElMessage({
                        type: "error",
                        message,
                        duration: 3000,
                    });
                }
                this.handleResponse?.(response)
                return response;
            },

            (error) => {
                const status = error.response?.status;

                switch (status) {
                    case 401:
                        ElMessage({
                            type: "error",
                            message: "登录信息过期，请重新登录",
                            duration: 3000,
                            onClose() {
                                localStorage.removeItem("isLoggedIn");
                                window.location.reload();
                            },
                        });
                        break;

                    case 500:
                    case 502:
                        ElMessage({
                            type: "error",
                            message: "服务器异常",
                            duration: 3000,
                        });
                        break;

                    case 504:
                        ElMessage({
                            type: "error",
                            message: "网络超时",
                            duration: 3000,
                        });
                        break;
                }
                return Promise.reject(error);
            }
        );
    }

    /** 通用请求 */
    private request<T>(
        method: Method,
        url: string,
        params?: any,
        option?: RequestOption
    ): Promise<T> {
        const {config, errorCallBack} = option || {};

        return new Promise((resolve, reject) => {
            this.service({
                method,
                url,
                data: method === Method.POST || method === Method.PUT ? params : null,
                params: method === Method.GET || method === Method.DELETE ? params : null,
                ...config,
            })
                .then((res) => resolve(res as unknown as T))
                .catch((error) => {
                    errorCallBack && errorCallBack(error);
                    reject(error);
                });
        });
    }

    get<T>(url: string, params?: any, option?: RequestOption) {
        return this.request<T>(Method.GET, url, params, option);
    }

    post<T>(url: string, params?: any, option?: RequestOption) {
        return this.request<T>(Method.POST, url, params, option);
    }

    put<T>(url: string, params?: any, option?: RequestOption) {
        return this.request<T>(Method.PUT, url, params, option);
    }

    delete<T>(url: string, params?: any, option?: RequestOption) {
        return this.request<T>(Method.DELETE, url, params, option);
    }

    /** 版本检查（自动刷新） */
    checkVersion() {
        const versionKey = "version";
        const local = JSON.parse(localStorage.getItem(versionKey));

        this.get<any>("version.json", {date: Date.now()}, {config: {baseURL: "/"}})
            .then((res) => {
                if (res.data?.buildDate !== local?.buildDate) {
                    localStorage.setItem(versionKey, JSON.stringify(res.data));
                    window.location.reload();
                }
            })
            .catch(() => {
            });
    }
}