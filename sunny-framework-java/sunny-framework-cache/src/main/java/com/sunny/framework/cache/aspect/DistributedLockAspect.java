package com.sunny.framework.cache.aspect;

import com.sunny.framework.cache.annotation.DistributedLock;
import com.sunny.framework.cache.exception.DistributedLockException;
import org.aspectj.lang.ProceedingJoinPoint;
import org.aspectj.lang.annotation.Around;
import org.aspectj.lang.annotation.Aspect;
import org.aspectj.lang.reflect.MethodSignature;
import org.redisson.api.RLock;
import org.redisson.api.RedissonClient;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.core.DefaultParameterNameDiscoverer;
import org.springframework.core.ParameterNameDiscoverer;
import org.springframework.expression.EvaluationContext;
import org.springframework.expression.Expression;
import org.springframework.expression.ExpressionParser;
import org.springframework.expression.spel.standard.SpelExpressionParser;
import org.springframework.expression.spel.support.StandardEvaluationContext;

import java.lang.reflect.Method;

/**
 * 分布式锁切面
 */
@Aspect
public class DistributedLockAspect {

    public static final Logger logger = LoggerFactory.getLogger(DistributedLockAspect.class);

    private final RedissonClient redissonClient;

    public DistributedLockAspect(RedissonClient redissonClient) {
        this.redissonClient = redissonClient;
    }

    /**
     * 生成分布式锁的key
     */
    public String generateKey(String key, String prefix, ProceedingJoinPoint joinPoint) {
        // 解析SpEL表达式
        if (key.contains("#")) {
            return prefix + parseSpel(key, joinPoint);
        }
        return prefix + key;
    }

    /**
     * 解析SpEL表达式
     */
    private String parseSpel(String spel, ProceedingJoinPoint joinPoint) {
        // 获取方法参数名和值
        MethodSignature signature = (MethodSignature) joinPoint.getSignature();
        Method method = signature.getMethod();
        Object[] args = joinPoint.getArgs();
        String[] parameterNames = getParameterNames(method);

        // 创建解析上下文
        EvaluationContext context = new StandardEvaluationContext();
        for (int i = 0; i < parameterNames.length; i++) {
            context.setVariable(parameterNames[i], args[i]);
        }

        // 解析SpEL
        ExpressionParser parser = new SpelExpressionParser();
        Expression expression = parser.parseExpression(spel);
        return expression.getValue(context, String.class);
    }

    /**
     * 获取方法参数名
     */
    private String[] getParameterNames(Method method) {
        ParameterNameDiscoverer discoverer = new DefaultParameterNameDiscoverer();
        String[] parameterNames = discoverer.getParameterNames(method);
        if (parameterNames == null) {
            parameterNames = new String[method.getParameterCount()];
            for (int i = 0; i < parameterNames.length; i++) {
                parameterNames[i] = "arg" + i;
            }
        }
        return parameterNames;
    }

    /**
     * 环绕通知
     */
    @Around("@annotation(distributedLock)")
    public Object around(ProceedingJoinPoint joinPoint, DistributedLock distributedLock) throws Throwable {
        // 生成锁的key
        String lockKey = generateKey(distributedLock.key(), distributedLock.prefix(), joinPoint);

        RLock lock = redissonClient.getLock(lockKey);
        boolean locked = false;

        logger.debug("尝试获取分布式锁，key: {}", lockKey);

        try {
            // 尝试获取锁
            locked = lock.tryLock(distributedLock.waitTime(), distributedLock.leaseTime(), distributedLock.timeUnit());

            if (!locked) {
                logger.warn("获取分布式锁失败，key: {}", lockKey);
                throw new DistributedLockException(distributedLock.errorMessage());
            }

            logger.debug("成功获取分布式锁，key: {}", lockKey);

            // 执行目标方法
            return joinPoint.proceed();

        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
            logger.error("获取分布式锁时被中断，key: {}", lockKey, e);
            throw new DistributedLockException("操作被中断");
        } catch (Exception e) {
            // 业务异常处理
            if (distributedLock.autoUnlockOnException()) {
                logger.error("业务执行异常，自动释放锁，key: {}", lockKey, e);
            } else {
                logger.error("业务执行异常，key: {}", lockKey, e);
            }
            throw e;
        } finally {
            // 释放锁
            if (locked && lock.isHeldByCurrentThread()) {
                lock.unlock();
                logger.debug("释放分布式锁，key: {}", lockKey);
            }
        }
    }
}