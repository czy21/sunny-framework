<template>
  <el-table-column :label="props.node.name" :prop="props.node.prop" header-align="center" v-if="props.node.children && props.node.children.length > 0">
    <template #header="scope">
      {{ renderHeader(props.node, scope) }}
    </template>
    <dynamic-el-column :node="child" v-for="child in props.node.children">
      <template #default="scope">
        <slot :prop="scope.prop" :scope="scope.scope"/>
      </template>
    </dynamic-el-column>
  </el-table-column>
  <el-table-column :label="props.node.name" :prop="props.node.prop" header-align="center" :fixed="props.node.fixed" :width="props.node.style?.width||props.node.width||''" v-else>
    <template #header="scope">
      {{ renderHeader(props.node, scope) }}
    </template>
    <template #default="scope">
      <slot :prop="props.node.prop" :scope="scope"/>
    </template>
  </el-table-column>
</template>

<script setup lang="tsx">
import {ElTableColumn, RenderRowData} from "element-plus";
import { TableColumn } from "./DynamicTable";

const props = defineProps(["node"])

const renderHeader = (node: TableColumn, scope: RenderRowData<any>) => {
  scope.column.params = node
  return scope.column.label
}
</script>