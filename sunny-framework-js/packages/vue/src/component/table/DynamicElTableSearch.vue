<template>
  <el-form ref="searchRef" :model="query" :inline="true">
    <el-form-item :label="item.name" :prop="item.prop" v-for="item in options">
      <el-input v-if="item.type === 'input'" v-model="query[item.prop]" :disabled="item.disabled" :placeholder="item.placeholder" clearable/>
      <el-select v-else-if="item.type === 'select'" v-model="query[item.prop]" :disabled="item.disabled" :placeholder="item.placeholder" clearable>
        <el-option v-for="opt in item.options" :label="opt.label" :value="opt.value"></el-option>
      </el-select>
      <el-date-picker v-else-if="item.type === 'date'" type="date" v-model="query[item.prop]" :value-format="item.format"/>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" :icon="Search" @click="search">搜索</el-button>
      <el-button :icon="Refresh" @click="resetForm(searchRef)">重置</el-button>
    </el-form-item>
  </el-form>
</template>

<script lang="ts" setup>
import {FormInstance} from 'element-plus';
import {Search, Refresh} from '@element-plus/icons-vue';
import {PropType, ref} from 'vue';
import {DynamicFormOptionList} from '../form/DynamicFormType.ts';

const props = defineProps({
  query: {
    type: Object,
    required: true
  },
  options: {
    type: Array as PropType<Array<DynamicFormOptionList>>,
    required: true
  },
  search: {
    type: Function,
    default: () => {
    }
  }
});

const searchRef = ref<FormInstance>();
const resetForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.resetFields()
  props.search();
}
</script>