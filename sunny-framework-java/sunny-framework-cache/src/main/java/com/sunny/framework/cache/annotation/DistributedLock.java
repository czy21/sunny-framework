package com.sunny.framework.cache.annotation;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;
import java.util.concurrent.TimeUnit;

/**
 * 分布式锁注解
 */
@Target(ElementType.METHOD)
@Retention(RetentionPolicy.RUNTIME)
public @interface DistributedLock {
    
    /**
     * 锁的key，支持SpEL表达式
     */
    String key();
    
    /**
     * 锁的key前缀
     */
    String prefix() default "";
    
    /**
     * 等待锁的最长时间（秒）
     */
    long waitTime() default 5;
    
    /**
     * 锁的持有时间（秒）
     */
    long leaseTime() default 30;
    
    /**
     * 时间单位
     */
    TimeUnit timeUnit() default TimeUnit.SECONDS;
    
    /**
     * 获取锁失败时的错误消息
     */
    String errorMessage() default "系统繁忙，请稍后重试";
    
    /**
     * 是否在业务异常时自动释放锁
     */
    boolean autoUnlockOnException() default true;
}