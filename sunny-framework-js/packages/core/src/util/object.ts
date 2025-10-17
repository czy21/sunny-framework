import _ from 'lodash'

export const isEmpty = (value: any) => (!value && value !== 0) || ((_.isObject(value) || _.isArray(value)) && _.isEmpty(value))

export const isNotEmpty = (value: any) => !isEmpty(value)

export const getValueByExpression = (obj: any, expression: string) => {
    try {
        return Function("obj", "return " + expression)(obj)
    } catch (ex) {
        console.log("getValueByExpression", ex)
    }
}