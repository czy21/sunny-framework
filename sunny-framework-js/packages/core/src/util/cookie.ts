import Cookies, {CookieAttributes} from "js-cookie";

export const getToken = (tokenKey?: string = "token") => {
    return Cookies.get(tokenKey)
}

export const setToken = (value: string, options?: CookieAttributes, tokenKey?: string = "token") => {
    Cookies.set(tokenKey, value, {domain: '.' + window.location.hostname.split('.').slice(-2).join('.'), ...options});
}

export const delToken = (options?: CookieAttributes, tokenKey?: string = "token") => {
    Cookies.remove(tokenKey, {domain: '.' + window.location.hostname.split('.').slice(-2).join('.'), ...options});
}