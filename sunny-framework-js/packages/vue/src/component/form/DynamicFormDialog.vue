<template>
  <el-dialog :model-value="props.dialogShow" :title="props.dialogTitle" @close="handleCancel">
    <DynamicForm ref="editRef" :form-data="props.formData" :option="props.formOption" @submit="handleSubmit" @cancel="handleCancel"/>
  </el-dialog>
</template>

<script lang="ts" setup>
import {ref, getCurrentInstance} from "vue";
import {DynamicForm, DynamicFormOption} from "@sunny-framework-js/vue";
import type {FormInstance} from 'element-plus'

const editRef = ref();

const props = defineProps<{
  dialogShow: boolean,
  dialogTitle: string,
  formOption: DynamicFormOption,
  formData: Record<string, any>
}>()

const emit = defineEmits<{
  'update:dialogShow': [value: boolean]
  'submit': [formEl: FormInstance]
  'cancel': [formEl: FormInstance]
}>()

const instance = getCurrentInstance();

const hasListener = (eventName: string) => {
  const propName = `on${eventName.charAt(0).toUpperCase() + eventName.slice(1)}`;
  return !!instance?.vnode.props?.[propName];
};

const handleSubmit = () => {
  emit('submit', editRef.value.formRef)
}

const handleCancel = () => {
  if (!hasListener('cancel')) {
    editRef.value.formRef.clearValidate()
    emit('update:dialogShow', false)
    return
  }
  emit('cancel', editRef.value.formRef)
}

</script>