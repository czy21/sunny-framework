import _ from 'lodash'
import {util} from '@sunny-framework-js/core'
import {ElMessageBox} from "element-plus";

export const inform = function (text: string, callback?: Function) {
    ElMessageBox.alert(text, '提示', {
        type: 'info',
        dangerouslyUseHTMLString: true,
        showClose: false
    }).then(() => {
        util.basic.callIfExists(callback)
    })
}

export const warn = function (text: string, callback?: Function) {
    ElMessageBox.alert(text, '警告', {
        type: 'warning',
        dangerouslyUseHTMLString: true,
        showClose: false
    }).then(() => {
        util.basic.callIfExists(callback)
    })
}

export const confirm = function (text: string, successCallback?: Function, cancelCallback?: Function) {
    ElMessageBox.confirm(text, '提示', {
        dangerouslyUseHTMLString: true,
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
    }).then(() => {
        util.basic.callIfExists(successCallback)
    }).catch(() => {
        util.basic.callIfExists(cancelCallback)
    })
}

export const validateForm = function (formEl: any, callback?: Function) {
    if (!formEl) return
    formEl.validate((valid: any) => {
        if(valid) {
            util.basic.callIfExists(callback)
        }
    })
}

// export const validateForm = function (vm: any, target: string) {
//     let res = false
//     vm.$refs[target].validate((valid: boolean) => {
//         res = valid
//     })
//     return res
// }
//
// export const actWithValidation = function (vm: any, target: Array<String> | String, callback?: Function) {
//
//     let targets = _.isString(target) ? new Array(target) : target
//
//     let valid = _.every(targets, (v: string) => {
//         return validateForm(vm, v)
//     })
//     valid ? util.basic.callIfExists(callback) : warn("请检查输入的参数再执行操作")
// }