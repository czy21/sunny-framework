<template>
  <el-dialog :model-value="props.dialogShow" :title="props.dialogTitle" @close="handleCancel">
    <DynamicForm ref="editRef" :form-data="props.formData" :option="props.formOption" @submit="handleSubmit" @cancel="handleCancel"/>
  </el-dialog>
</template>

<script lang="ts" setup>
import {ref} from "vue";
import {DynamicForm, DynamicFormOption} from "@sunny-framework-js/vue";
import type {FormInstance} from 'element-plus'

const editRef = ref();

interface Props {
  dialogShow: boolean
  dialogTitle?: string
  formOption?: DynamicFormOption,
  formData?: Partial<FormData>
}

const props = withDefaults(defineProps<Props>(), {
  dialogShow: false,
  dialogTitle: '',
  formOption: (): any => {
  },
  formData: (): any => {
  },
})

const emit = defineEmits<{
  'update:dialogShow': [value: boolean]
  'submit': [formEl: FormInstance]
  'cancel': [formEl: FormInstance]
}>()

const handleSubmit = () => {
  emit('submit', editRef.value.formRef)
}

const handleCancel = () => {
  emit('cancel', editRef.value.formRef)
}

</script>