<template>
    <el-pagination
        class="pagination"
        v-model:current-page="page"
        v-model:page-size="pageSize"
        :total="total"
        :background="background"
        :layout="layout"
        :page-sizes="pageSizes"
        @current-change="handlePage"
        @size-change="handlePageSize"
    />
</template>

<script setup lang="ts">
import {watch, PropType} from "vue"

const props = defineProps({
  total: {
    type: Number as PropType<number>,
    default: 0,
  },
  pageSizes: {
    type: Array as PropType<number[]>,
    default() {
      return [10, 20, 30, 50];
    },
  },
  layout: {
    type: String,
    default: "total, sizes, prev, pager, next, jumper",
  },
  background: {
    type: Boolean,
    default: true,
  }
});

const emit = defineEmits(["pagination"]);

const page = defineModel("page", {
  type: Number,
  required: true,
  default: 1,
});

const pageSize = defineModel("pageSize", {
  type: Number,
  required: true,
  default: 10,
});

const handlePage = (val: number) => {
  emit('pagination', {page: val, pageSize: pageSize.value})
}

const handlePageSize = (val: number) => {
  page.value = 1
  emit('pagination', {page: page.value, pageSize: val})
}
</script>

<style lang="scss" scoped>
.pagination {
  padding: 12px;
}
</style>