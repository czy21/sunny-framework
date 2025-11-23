package com.sunny.framework.cache.service;


import org.redisson.api.RedissonClient;


public class DistributeLockService {

    RedissonClient redissonClient;

    public DistributeLockService(RedissonClient redissonClient) {

    }

}
