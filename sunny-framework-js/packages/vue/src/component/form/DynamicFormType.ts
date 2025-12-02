import type {FormItemRule} from 'element-plus'

export interface DynamicFormOption {
    labelPosition?: "top" | "bottom" | "left" | "right";
    submitShow?: boolean
    submitText?: string
    cancelText?: string
    span?: string
    cols?: number
    items: DynamicFormItem[]
}

export interface DynamicFormItem {
    prop: string
    name: string
    type: string
    placeholder?: string
    disabled?: boolean | ((record?: any) => boolean)
    options?: DynamicFormItemOption[]
    props?: DynamicFormItemProps
    format?: string
    rules?: FormItemRule[]
}

export interface DynamicFormItemOption {
    label: string | ((record?: any) => string)
    value: any
}

export interface DynamicFormItemProps {
    label: string | ((record?: any) => string)
    value: any
}