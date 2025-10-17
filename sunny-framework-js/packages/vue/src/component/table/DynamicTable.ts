import {CSSProperties} from "vue";
import {FormItemRule} from 'element-plus'
import {VxeTableDefines} from 'vxe-table'

declare module 'element-plus' {
    export interface TableColumnCtx<T> {
        params: TableColumn
    }
}

export interface TableProps {
    defaultRowValue?: object,
    columns: Array<TableColumn>,
    data: Array<object>,
    dict?: DictType,
    subTotal?: Array<SubTotalType>
    rules?: { [key: string]: FormItemRule[] } | { [key: string]: VxeTableDefines.ValidatorRule[] }
    editable?: boolean
    showSummary?: boolean
    showAddRow?: boolean
}

export interface TableEmits {
    handleEdit: [value: any, scope: any, dict: DictType]
    handleEditChange: [value: any, scope: any, dict: DictType]
    handleSelectSearch: [value: any, scope: any, dict: DictType]
}

export interface TableColumn {
    prop: string
    name: string
    type?: string | "string" | "number" | "select"
    parentProp?: string
    parentName?: string
    required?: boolean
    editable?: boolean | string
    dictKey?: string
    dictPush?: DictPush
    multiple?: boolean
    dictOnlyOneDefaultSelect?: boolean,
    rowTotal?: string
    colTotal?: boolean
    changeByProps?: string[]
    heads?: (string | TableHead)[]
    width?: number | string
    fixed?: string
    remote?: boolean
    min?: number
    max?: number
    precision?: number
    custom?: boolean
}

export interface TableHead {
    name: string
    style?: CSSProperties
}

export interface DictType {
    [key: string]: Array<{ label: string, value: Object, extra: Object }>
}

export interface DictPush {
    [key: string]: string
}

export interface SubTotalType {
    key: string

    groupBy(item: Object, data?: { columns: any[], data: any[] }): boolean

    byValue: boolean
}