export interface DynamicFormOption {
    labelPosition: "top" | "bottom" | "left" | "right";
    span: string;
    items: DynamicFormOptionList[]
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