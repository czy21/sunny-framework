<template>
  <n-config-provider>
    <n-data-table ref="tableRef"
                  :columns="columns"
                  :data="props.data"
                  :max-height="500"
                  :scroll-x="1800"
                  virtual-scroll
                  :single-line="false"
                  size="small"
                  :render-cell="(value,row,column)=>handleRenderCell({row,column})"
    />
  </n-config-provider>
</template>

<script setup lang="tsx">
import {FunctionalComponent, ref, computed, h} from "vue"
import {util} from "@sunny-framework-js/core"
import {TableProps, TableEmits} from "./DynamicTable.ts";
import {NDataTable} from "naive-ui";
import {ElButton, ElDatePicker, ElInput, ElInputNumber, ElOption, ElSelect} from "element-plus";
import _ from "lodash";

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

function recursiveColumn(node: any) {
  const newNode: any = {
    key: node.prop,
    property: node.prop,
    title: node.name,
    titleAlign: "center",
    resizable: true,
    minWidth: 100,
    width: node.style?.width || node.width || 150,
    fixed: node.fixed,
    params: _.omit(node, ["children"])
  }
  if (newNode.params.type === 'index') {
    newNode.render = (row, rowIndex) => {
      row[newNode.property] = rowIndex + 1
      return row[newNode.property]
    }
  }
  for (let child of (node.children || [])) {
    newNode.children = newNode.children || []
    newNode.children.push(recursiveColumn(child));
  }
  return newNode;
}

let columns = computed(() => recursiveColumn({children: props.columns}).children)


const handleRenderCell = (scope) => {
  let label = scope.row[scope.column.property]
  if (scope.column.params.dictKey && props.dict) {
    const options = props.dict[scope.column.params.dictKey]
    let value = options?.find(t => t.value === scope.row[scope.column.property])?.label
    if (value) {
      label = value
    }
    // if (!label && !value && options?.length === 1 && scope.column.params.dictOnlyOneDefaultSelect) {
    //   scope.row[scope.column.property] = options[0].value
    //   handleExtra(scope.row[scope.column.property], scope)
    // }
  }
  if (scope.column.params.type === 'number') {
    label = util.number.toMilliSeparator(label, true, !util.object.isEmpty(scope.column.params.precision) ? scope.column.params.precision : 2)
  }
  return label
}

</script>

<style scoped lang="scss">
:deep(.n-data-table .n-data-table-td.n-data-table-td--last-row) {
  border-bottom: 1px solid var(--n-merged-border-color);
}
</style>