import { AxiosResponse } from "axios";
import _ from 'lodash'


export const callIfExists = (fn?: Function, resultForNonFunction?: any, ...args: any) => {
    if (_.isFunction(fn)) {
        return fn?.apply(args)
    }
    return resultForNonFunction
}

export const downloadFile = (res: AxiosResponse, fileName?: string) => {
    let url = URL.createObjectURL(res.data)
    const a = document.createElement('a')
    a.style.display = 'none'
    a.download = decodeURIComponent(res.headers?.filename ?? fileName)
    a.href = url
    a.click()
    URL.revokeObjectURL(url);
}