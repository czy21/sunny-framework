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

export const createPolling = ({ task, interval = 2000, timeout, immediate = true, stopCondition, continueOnError = false, }) => {

    let timer: any = null;
    let active = false;
    let startTime = 0;

    const stop = () => {
        active = false;
        clearTimeout(timer);
        timer = null;
    };

    const exec = async () => {
        if (!active) return;

        if (timeout && Date.now() - startTime >= timeout) {
            stop();
            throw new Error("Polling timeout");
        }

        try {
            const res = await task();

            if (stopCondition?.(res)) {
                stop();
                return;
            }
        } catch (e) {
            if (!continueOnError) {
                stop();
                throw e;
            }
        }

        timer = setTimeout(exec, interval);
    };

    const start = () => {
        if (active) return;
        active = true;
        startTime = Date.now();
        immediate ? exec() : (timer = setTimeout(exec, interval));
    };

    return { start, stop, isActive: () => active };
}
