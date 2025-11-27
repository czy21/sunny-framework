<template>
  <el-tooltip ref="popperRef" :visible="visible" effect="light" pure trigger="click"
              role="dialog" teleported class="cron-picker-popper" placement="bottom-start" :hide-after="0" persistent>
    <template #default>
      <el-input v-model="display" readonly placeholder="选择定时任务" @click.stop="open" class="cron-picker-input"/>
    </template>

    <template #content>
      <div class="cron-panel" @mousedown.stop>
        <el-tabs v-model="mode" tab-position="top" class="cron-tabs">
          <!-- 每天 -->
          <el-tab-pane label="每天" name="day">
            <el-row class="cron-row" align="middle">
              <el-col :span="6">时段</el-col>
              <el-col :span="18">
                <el-radio-group v-model="period" size="small">
                  <el-radio label="midnight">凌晨</el-radio>
                  <el-radio label="morning">上午</el-radio>
                  <el-radio label="afternoon">下午</el-radio>
                  <el-radio label="evening">晚上</el-radio>
                </el-radio-group>
              </el-col>
            </el-row>
            <el-row class="cron-row" align="middle">
              <el-col :span="6">时间</el-col>
              <el-col :span="18">
                <el-time-select v-model="time" :start="periodStart[period]" :end="periodEnd[period]"
                                step="00:15" :editable="false" :clearable="false" style="width:100%;"/>
              </el-col>
            </el-row>
          </el-tab-pane>

          <!-- 每周 -->
          <el-tab-pane label="每周" name="week">
            <el-row class="cron-row" align="middle">
              <el-col :span="6">星期</el-col>
              <el-col :span="18">
                <el-select v-model="weekdaysSelected" multiple clearable placeholder="选择星期" @clear="onWeekClear" style="width:100%;">
                  <el-option v-for="w in weekdays" :key="w.value" :label="w.label" :value="w.value"/>
                </el-select>
              </el-col>
            </el-row>
            <el-row class="cron-row" align="middle">
              <el-col :span="6">时段</el-col>
              <el-col :span="18">
                <el-radio-group v-model="period" size="small">
                  <el-radio label="midnight">凌晨</el-radio>
                  <el-radio label="morning">上午</el-radio>
                  <el-radio label="afternoon">下午</el-radio>
                  <el-radio label="evening">晚上</el-radio>
                </el-radio-group>
              </el-col>
            </el-row>
            <el-row class="cron-row" align="middle">
              <el-col :span="6">时间</el-col>
              <el-col :span="18">
                <el-time-select v-model="time" :start="periodStart[period]" :end="periodEnd[period]"
                                step="00:15" :editable="false" :clearable="false" style="width:100%;"/>
              </el-col>
            </el-row>
          </el-tab-pane>

          <!-- 每月 -->
          <el-tab-pane label="每月" name="month">
            <el-row class="cron-row" align="middle">
              <el-col :span="6">日期</el-col>
              <el-col :span="18">
                <el-select v-model="monthDaysSelected" multiple clearable placeholder="选择日期" @clear="onMonthClear" style="width:100%;">
                  <el-option v-for="i in 31" :key="i" :label="i" :value="i"/>
                </el-select>
              </el-col>
            </el-row>
            <el-row class="cron-row" align="middle">
              <el-col :span="6">时段</el-col>
              <el-col :span="18">
                <el-radio-group v-model="period" size="small">
                  <el-radio label="midnight">凌晨</el-radio>
                  <el-radio label="morning">上午</el-radio>
                  <el-radio label="afternoon">下午</el-radio>
                  <el-radio label="evening">晚上</el-radio>
                </el-radio-group>
              </el-col>
            </el-row>
            <el-row class="cron-row" align="middle">
              <el-col :span="6">时间</el-col>
              <el-col :span="18">
                <el-time-select v-model="time" :start="periodStart[period]" :end="periodEnd[period]"
                                step="00:15" :editable="false" :clearable="false" style="width:100%;"/>
              </el-col>
            </el-row>
          </el-tab-pane>

          <!-- 指定时间 -->
          <el-tab-pane label="指定时间" name="time">
            <el-date-picker v-model="dateTime" type="datetime" placeholder="选择日期时间"
                            format="YYYY-MM-DD HH:mm" value-format="YYYY-MM-DD HH:mm" style="width:100%;"/>
          </el-tab-pane>
        </el-tabs>

        <!-- Cron & 下一次运行 -->
        <div class="cron-display-card">
          <el-input v-model="cronDisplay" readonly style="margin-bottom:6px;"/>
          <ul v-if="nextRuns.length">
            <li v-for="(v,i) in nextRuns" :key="i">{{ v }}</li>
          </ul>
        </div>

        <!-- 操作按钮 -->
        <div class="cron-footer">
          <el-button size="small" @click="clear">清除</el-button>
          <el-button size="small" @click="close">取消</el-button>
          <el-button size="small" type="primary" @click="confirm">确定</el-button>
        </div>
      </div>
    </template>
  </el-tooltip>
</template>

<script setup lang="ts">
import {ref, computed, watch} from 'vue'
import {ElMessage} from 'element-plus'
import later from '@breejs/later'

later.date.localTime()

const props = defineProps({modelValue: String})
const emit = defineEmits(['update:modelValue', 'change'])

const visible = ref(false)
const mode = ref<'day' | 'week' | 'month' | 'time'>('day')
const period = ref<'midnight' | 'morning' | 'afternoon' | 'evening'>('morning')
const time = ref('08:00')
const weekdaysSelected = ref<string[]>(['1'])
const monthDaysSelected = ref<number[]>([1])
const dateTime = ref('')

const weekdays = [
  {label: '周一', value: '1'}, {label: '周二', value: '2'}, {label: '周三', value: '3'},
  {label: '周四', value: '4'}, {label: '周五', value: '5'}, {label: '周六', value: '6'}, {label: '周日', value: '0'},
]

const periodStart = {midnight: '00:00', morning: '08:00', afternoon: '12:00', evening: '18:00'}
const periodEnd = {midnight: '07:59', morning: '11:59', afternoon: '17:59', evening: '23:59'}

const onWeekClear = () => weekdaysSelected.value = ['1']
const onMonthClear = () => monthDaysSelected.value = [1]

watch(mode, m => {
  period.value = 'morning'
  time.value = periodStart[period.value]
  if (m !== 'week') weekdaysSelected.value = []
  if (m !== 'month') monthDaysSelected.value = []
  if (m !== 'time') dateTime.value = ''
  if (m === 'week' && weekdaysSelected.value.length === 0) weekdaysSelected.value = ['1']
  if (m === 'month' && monthDaysSelected.value.length === 0) monthDaysSelected.value = [1]
})
watch(period, p => time.value = periodStart[p])

const display = computed(() => {
  if (mode.value === 'day') return `每天 ${time.value}`
  if (mode.value === 'week') return `每周 ${weekdaysSelected.value.join(',')} ${time.value}`
  if (mode.value === 'month') return `每月 ${monthDaysSelected.value.join(',')} ${time.value}`
  if (mode.value === 'time' && dateTime.value) {
    const d = new Date(dateTime.value)
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
  }
  return ''
})

const cronDisplay = computed(() => {
  const [h, m] = time.value.split(':')
  if (mode.value === 'day') return `0 ${m} ${h} * * ?`
  if (mode.value === 'week') {
    const w = weekdaysSelected.value.map(w => w === '0' ? '1' : String(Number(w) + 1))
    return `0 ${m} ${h} ? * ${w.join(',')}`
  }
  if (mode.value === 'month') return `0 ${m} ${h} ${monthDaysSelected.value.join(',')} * ?`
  if (mode.value === 'time' && dateTime.value) {
    const d = new Date(dateTime.value)
    return `${d.getSeconds()} ${d.getMinutes()} ${d.getHours()} ${d.getDate()} ${d.getMonth() + 1} ?`
  }
  return ''
})

function quartzToLater(cron: string) {
  const parts = cron.trim().split(/\s+/)
  if (parts.length < 6) return null
  const [sec, min, hour, day, month, week] = parts
  const weekMap: Record<string, string> = {'1': 'SUN', '2': 'MON', '3': 'TUE', '4': 'WED', '5': 'THU', '6': 'FRI', '7': 'SAT'}
  const weekExpr = week === '?' ? '' : week.split(',').map(w => weekMap[w] || w).join(',')
  const dayExpr = day === '?' ? '' : day
  return [sec, min, hour, dayExpr || '?', month, weekExpr || '?'].join(' ')
}

const nextRuns = computed(() => {
  if (!cronDisplay.value) return []
  const laterCron = quartzToLater(cronDisplay.value)
  if (!laterCron) return []
  try {
    const sched = later.parse.cron(laterCron, true)
    const dates = later.schedule(sched).next(5)
    return dates.map(d => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`)
  } catch {
    return []
  }
})

const open = () => visible.value = true
const close = () => visible.value = false
const clear = () => {
  period.value = 'morning'
  time.value = periodStart[period.value]
  weekdaysSelected.value = ['1']
  monthDaysSelected.value = [1]
  dateTime.value = ''
}
const confirm = () => {
  if (mode.value === 'week' && weekdaysSelected.value.length === 0) return ElMessage.warning('每周任务必须至少选择一个星期')
  if (mode.value === 'month' && monthDaysSelected.value.length === 0) return ElMessage.warning('每月任务必须至少选择一个日期')
  emit('update:modelValue', cronDisplay.value)
  emit('change', cronDisplay.value)
  close()
}

// 初始化 modelValue
watch(() => props.modelValue, val => {
  if (!val) return
  const p = val.split(' ')
  if (p.length < 6) return
  const [sec, min, hour, day, mon, week] = p
  if (day === '*' && mon === '*' && week === '?') mode.value = 'day', time.value = `${hour}:${min}`
  else if (day === '?' && mon === '*') mode.value = 'week', weekdaysSelected.value = week.split(',').map(w => w === '1' ? '0' : String(Number(w) - 1)), time.value = `${hour}:${min}`
  else if (mon === '*' && week === '?') mode.value = 'month', monthDaysSelected.value = day.split(',').map(Number), time.value = `${hour}:${min}`
  else mode.value = 'time', dateTime.value = `${new Date().getFullYear()}-${String(mon).padStart(2, '0')}-${String(day).padStart(2, '0')} ${hour}:${min}`
}, {immediate: true})
</script>

<style scoped>
.cron-panel {
  width: 500px;
  padding: 12px;
  background: #fff;
  border-radius: 6px;
  box-shadow: 0 6px 12px rgba(0, 0, 0, .15)
}

.cron-row {
  margin-bottom: 8px
}

.cron-display-card {
  margin-top: 12px;
  padding: 10px;
  background: #f9f9f9;
  border-radius: 4px
}

.cron-footer {
  display: flex;
  justify-content: flex-end;
  margin-top: 12px;
  gap: 8px
}
</style>
