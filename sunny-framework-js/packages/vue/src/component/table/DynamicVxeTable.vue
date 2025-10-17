<template>
  <vxe-table ref="tableRef"
             :data="props.data"
             :header-cell-style="headerCellStyle"
             @cell-click="handleCell"
             :edit-rules="props.rules"
             border
             width="100%"
             height="100%"
             size="mini"
             :column-config="{ resizable: true }"
             :scroll-x="{ enabled: true, gt: 0 }"
             :scroll-y="{ enabled: true, gt: 0 }"
             show-overflow
             :show-footer="props.showSummary"
             :footer-method="summaryMethod"
             @scroll="handleScroll"
  >
    <dynamic-vxe-column :node="t" v-for="t in props.columns">
      <template #default="{ prop, scope }">
        <slot :name="prop" :=scope v-if="scope.column.params.custom"/>
        <template v-else-if="scope.row[`${scope.column.property}_editable`]">
          <el-input ref="editRef" v-model="scope.row[scope.column.property]" @blur="onExitEditMode(scope)" v-if="isInput(scope)" @change="(value)=>handleInput(value,scope)"/>
          <el-input-number ref="editRef" v-model="scope.row[scope.column.property]" @blur="onExitEditMode(scope)" v-else-if="isInputNumber(scope)" @change="(value)=>handleInput(value,scope)"
                           size="default"
                           :controls="false"
                           :type="scope.column.params.type"
                           :precision="util.object.isNotEmpty(scope.column.params.precision)?scope.column.params.precision : 2"
                           :min="scope.column.params.min !==null?scope.column.params.min : -Infinity"
                           :max="scope.column.params.max !==null?scope.column.params.max : Infinity"
          />
          <el-select ref="editRef" v-model="scope.row[`${scope.column.property}${scope.column.params.multiple?'List':''}`]" @blur="onExitEditMode(scope)" v-else-if="isSelect(scope)" @change="(value) => handleSelect(value, scope)"
                     clearable
                     filterable
                     :multiple="scope.column.params.multiple"
                     :remote="scope.column.params.remote"
                     :remote-method="(value)=>emit('handleSelectSearch',value,scope,props.dict)"
          >
            <el-option v-for="t in props.dict[scope.column.params.dictKey]" :label="t.label" :value="t.value"/>
          </el-select>
          <el-date-picker ref="editRef" v-model="scope.row[scope.column.property]" @blur="onExitEditMode(scope)" v-else-if="isDate(scope)" @change="(value)=>handleDate(value,scope)"
                          size="default"
                          :type="scope.column.params.type"
                          :value-format="scope.column.params.format || 'YYYY-MM-DD HH:mm:ss'"
          />
        </template>
        <span v-else-if="scope.column.property === 'action'">
          <slot :name="scope.column.property" :=scope></slot>
          <el-button @click="addRow(scope)" link type="primary" v-if="props.showAddRow && showAddRow(scope)">加行</el-button>
          <el-button @click="delRow(scope)" link type="danger" v-if="props.showAddRow">删除</el-button>
        </span>
        <show-cell :="scope" v-else/>
      </template>
    </dynamic-vxe-column>
    <template #empty v-if="props.showAddRow">
      <el-button @click="addRow" type="primary">加行</el-button>
    </template>
  </vxe-table>
</template>

<script lang="tsx" setup>
import {FunctionalComponent, ref} from "vue"
import DynamicVxeColumn from "./DynamicVxeColumn.vue"
import {util} from "@sunny-framework-js/core"
import {TableProps, TableEmits} from "./DynamicTable.ts";
import {VxeTable} from "vxe-table";
import {ElButton, ElDatePicker, ElInput, ElInputNumber, ElOption, ElSelect} from "element-plus";

const props = withDefaults(defineProps<TableProps>(), {
  defaultRowValue() {
    return {}
  },
  columns: () => [],
  data: () => [],
  dict() {
    return {}
  },
  rules() {
    return {}
  },
  subTotal() {
    return []
  },
  editable() {
    return false
  },
  showSummary() {
    return true
  },
  showAddRow() {
    return true
  }
})

const emit = defineEmits<TableEmits>()

const tableRef = ref()
const editRef = ref()

const headerCellStyle = ({column}) => {
  return column.params.style
}

const handleCellFocus = (callBackFn?: Function) => {
  editRef.value?.forEach((t, i, a) => {
    if (i === a.length - 1) {
      t.focus?.()
      callBackFn && callBackFn(t)
    }
  })
}

const isInput = (scope) => {
  let val = !scope.column.params.type || scope.column.params.type === 'string'
  if (val) {
    handleCellFocus()
  }
  return val
}

const isInputNumber = (scope) => {
  let val = scope.column.params.type === 'number'
  if (val) {
    handleCellFocus()
  }
  return val
}

const isSelect = (scope) => {
  let val = scope.column.params.type === 'select'
  if (val) {
    handleCellFocus()
  }
  return val
}

const isDate = (scope) => {
  let val = scope.column.params.type === 'date' || scope.column.params.type === 'datetime'
  if (val) {
    handleCellFocus(t => t.handleOpen())
  }
  return val
}

const onExitEditMode = (scope) => {
  delete scope.row[`${scope.column.property}_editable`]
  if (scope.column.params.type == 'number') {
    tableRef.value.updateFooter?.()
  }
}

const ShowCell: FunctionalComponent<any> = (scope) => {
  let label = scope.row[scope.column.property]
  if (scope.column.params.dictKey && props.dict) {
    const options = props.dict[scope.column.params.dictKey]
    let value = options?.find(t => t.value === scope.row[scope.column.property])?.label
    if (value) {
      label = value
    }
    if (!label && !value && options?.length === 1 && scope.column.params.dictOnlyOneDefaultSelect) {
      scope.row[scope.column.property] = options[0].value
      handleExtra(scope.row[scope.column.property], scope)
    }
  }
  if (scope.column.params.type === 'index') {
    label = scope.row[scope.column.property] = scope.rowIndex + 1
  }
  if (scope.column.params.type === 'number') {
    label = util.number.toMilliSeparator(label, true, util.object.isNotEmpty(scope.column.params.precision) ? scope.column.params.precision : 2)
  }
  return label
}

const handleCell = (scope) => {
  if (props.editable && (scope.column.params.editable == true || util.object.getValueByExpression(scope.row, scope.column.params.editable))) {
    scope.row[`${scope.column.property}_editable`] = true
    if (scope.column.params.multiple && scope.row[scope.column.property]) {
      scope.row[`${scope.column.property}List`] = scope.row[scope.column.property].split(" ")
    }
    emit("handleEdit", scope.row[scope.column.property], scope, props.dict)
  }
}

const changeColumn = (value, scope) => {
  const changeColumns = scope.$table.getColumns().filter(t => t.params.changeByProps?.includes(scope.column.property))
  changeColumns.forEach(c => {
    if (c.params.rowTotal) {
      scope.row[c.property] = Number(util.object.getValueByExpression(scope.row, c.params.rowTotal) || null).toFixed(util.object.isNotEmpty(c.params.precision) ? c.params.precision : 2)
    }
  })
}

const handleInput = (value: any, scope) => {
  changeColumn(value, scope)
  emit('handleEditChange', value, scope, props.dict)
}

const handleExtra = (value: any, scope) => {
  let dictPush = scope.column.params.dictPush
  if (dictPush) {
    let dictExtra = (props.dict[scope.column.params.dictKey].find((t: any) => t.value === value)?.extra || {})
    Object.entries(dictPush).forEach(([k, v]) => scope.row[k] = dictExtra[v])
  }
}

const handleSelect = (value: any, scope) => {
  handleExtra(value, scope)
  changeColumn(value, scope)
  if (scope.column.params.multiple) {
    scope.row[scope.column.property] = value.join(" ")
  }
  emit('handleEditChange', value, scope, props.dict)
}

const handleDate = (value: any, scope) => {
  changeColumn(value, scope)
  emit('handleEditChange', value, scope, props.dict)
}

const showAddRow = (scope) => {
  return scope.rowIndex == props.data.length - 1
}

const addRow = (scope) => {
  props.data.splice(scope?.rowIndex + 1, 0, {...props.defaultRowValue})
}

const delRow = (scope) => {
  props.data.splice(scope.rowIndex, 1)
}

const handleScroll = () => {
  editRef.value?.forEach(t => t.blur?.())
}

const summaryMethod = (data: { columns: any[], data: any[] }) => {
  const sums: any[] = []
  const subTotal = new Map((props.subTotal || []).map(t => [t.key, t]));
  if (data.columns && data.columns.length > 0) {
    const firstProperty = data.columns[0].property
    let reduceColumns = data.columns.filter(c => c.params.rowTotal || c.params.colTotal)

    reduceColumns.filter(c => c.params.rowTotal && util.object.isEmpty(c.params.changeByProps)).forEach(c => {
      data.data.forEach(t => t[c.property] = Number(util.object.getValueByExpression(t, c.params.rowTotal) || null).toFixed(2))
    })

    function reduceTotal(objs, sort) {
      return Object.entries(objs).filter(([k, v]) => v.length > 0).map(([k, v]) => {
        let subItem: any = {}
        subItem[firstProperty] = k == 'undefined' ? '' : k
        subItem["sort"] = sort
        reduceColumns.forEach(c => subItem[c.property] = Number(v.reduce((a, b) => a + (Number(b[c.property]) || 0), 0)).toFixed(2))
        return subItem
      })
    }

    subTotal.keys().forEach((k, i) => {
      const s = subTotal.get(k)
      let objs = s.byValue ? Object.groupBy(data.data, t => s.groupBy(t, data)) : {[k]: data.data.filter(t => s.groupBy(t, data))}
      sums.push(...reduceTotal(objs, i))
    })

    sums.push(...reduceTotal({"合计": data.data}, sums.length + 1))

    sums.sort((a, b) => a.sort - b.sort)
    reduceColumns.forEach(c => sums.forEach(t => t[c.property] = util.number.toMilliSeparator(t[c.property])))
  }
  return sums
}

defineExpose({
  tableRef
})

</script>
<style scoped lang="scss">
:deep(.el-input-number) {
  width: 100%;
}
</style>