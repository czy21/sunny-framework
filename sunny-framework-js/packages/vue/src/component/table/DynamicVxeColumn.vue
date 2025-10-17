<template>
  <vxe-colgroup :title="props.node.name" :field="props.node.prop" header-align="center" :params="props.node" v-if="props.node.children && props.node.children.length > 0">
    <dynamic-vxe-column :node="child" v-for="child in props.node.children">
      <template #default="scope">
        <slot :prop="scope.prop" :scope="scope.scope"/>
      </template>
    </dynamic-vxe-column>
  </vxe-colgroup>
  <vxe-column :title="props.node.name" :field="props.node.prop" header-align="center" :fixed="props.node.fixed" :width="props.node.style?.width||props.node.width||150" :params="props.node" v-else>
    <template #default="scope">
      <slot :prop="props.node.prop" :scope="scope"/>
    </template>
  </vxe-column>
</template>

<script setup lang="tsx">

const props = defineProps(["node"])

</script>