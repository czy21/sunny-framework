export interface DynamicFormOption {
    labelPosition?: "top" | "bottom" | "left" | "right";
    span?: string;
    items: DynamicFormItemOption[]
}

export interface DynamicFormItemOption {
    prop: string;
    name: string;
    type: string;
    placeholder?: string;
    disabled?: boolean;
    options?: any[];
    format?: string;
    rules?: any[]
}