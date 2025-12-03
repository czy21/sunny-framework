import type {FormItemRule} from 'element-plus'

export interface DynamicFormOption {
    labelPosition?: "top" | "bottom" | "left" | "right";
    submitShow?: boolean
    submitText?: string
    cancelText?: string
    span?: string
    items: DynamicFormItem[]
}

export interface DynamicFormItem {
    prop: string
    name: string
    type: string
    placeholder?: string
    checkStrictly?: boolean
    defaultExpandAll?: boolean
    disabled?: boolean | ((record?: any) => boolean)
    dictKey?: string
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
    label: string
    value: any
}