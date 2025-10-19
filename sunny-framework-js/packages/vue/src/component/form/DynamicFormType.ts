export interface DynamicFormOption {
    list: DynamicFormOptionList[];
    labelWidth?: number | string;
    span?: number;
}

export interface DynamicFormOptionList {
    prop: string;
    name: string;
    type: string;
    placeholder?: string;
    disabled?: boolean;
    options?: any[];
    format?: string;
    rules?: any[]
}