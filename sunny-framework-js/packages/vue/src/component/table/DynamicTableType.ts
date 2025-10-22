import {CSSProperties} from "vue";
import {FormItemRule} from 'element-plus'
import {VxeTableDefines} from 'vxe-table'

declare module 'element-plus' {
    export interface TableColumnCtx<T> {
        params: TableColumn
    }
}

export interface TableProps {
    defaultRowValue?: object
    selectable?: boolean
    columns: Array<TableColumn>
    data: Array<object>
    dict?: DictType
    subTotal?: Array<SubTotalType>
    rules?: { [key: string]: FormItemRule[] } | { [key: string]: VxeTableDefines.ValidatorRule[] }
    editable?: boolean
    showSummary?: boolean
    showAddRow?: boolean
    showPageable?: boolean
    page?: number
    pageSize?: number
    pageSizes?: number[]
    pageLayout?: string
    total?: number
}

export interface TableEmits {
    'selectable': [row: any, index: number]
    'edit': [value: any, scope: any, dict: DictType]
    'edit-change': [value: any, scope: any, dict: DictType]
    'select-search': [value: any, scope: any, dict: DictType]
    'pagination': [{ page: number, pageSize: number }]
}

export interface TableColumn {
    prop: string
    name: string
    type?: string | "string" | "number" | "select"
    selectable?: boolean
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