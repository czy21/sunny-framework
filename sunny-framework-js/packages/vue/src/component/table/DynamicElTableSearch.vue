<template>
  <el-form ref="formRef" :model="formData">
    <el-row :gutter="10">
      <el-col v-for="item in option.items" :span="item.span ?? option.span">
        <el-form-item v-if="item.type ==='action'">
          <el-button type="primary" :icon="Search" @click="handleSearch">搜索</el-button>
          <el-button :icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
        <el-form-item :label="item.name" :prop="item.prop" v-else>
          <el-input v-if="item.type === 'input'" v-model="formData[item.prop]" :disabled="item.disabled" :placeholder="item.placeholder" clearable/>
          <el-select v-else-if="item.type === 'select'" v-model="formData[item.prop]" :disabled="item.disabled" :placeholder="item.placeholder??''" clearable>
            <el-option v-for="opt in item.options" :label="opt.label" :value="opt.value"></el-option>
          </el-select>
          <el-date-picker v-else-if="item.type === 'date'" type="date" v-model="formData[item.prop]" :value-format="item.format"/>
        </el-form-item>
      </el-col>
    </el-row>
  </el-form>
</template>

<script lang="ts" setup>
import type {FormInstance} from 'element-plus'
import {Refresh, Search} from '@element-plus/icons-vue';
import {computed, ref} from 'vue';
import {DynamicFormOption} from '../form/DynamicFormType.ts';

const props = defineProps<{
  option: DynamicFormOption;
  formData: Record<string, any>;
}>();

const option = computed(() => {
  const opt = {
    span: 4,
    ...props.option
  }

  return {
    ...opt,
    items: [
      ...opt.items,
      {type: 'action'}
    ]
  }
})

const formData: Record<string, any> = computed(() => props.formData);

const emit = defineEmits<{
  'search': []
  'reset': []
}>()

const formRef = ref<FormInstance>()

const handleSearch = () => {
  emit('search')
}

const handleReset = () => {
  if (!formRef.value) return
  formRef.value.resetFields()
  handleSearch()
}

defineExpose({
  formRef
})

</script>