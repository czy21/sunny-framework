import { isEmpty } from "./object";


export const toMilliSeparator = (value: any, original?: boolean, fractionDigits: number = 2) => {
    let valueNumber = Number(value)
    if (isEmpty(value) || (!valueNumber && original)) {
        return value
    }
    return (valueNumber || Number(0)).toFixed(fractionDigits).replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}