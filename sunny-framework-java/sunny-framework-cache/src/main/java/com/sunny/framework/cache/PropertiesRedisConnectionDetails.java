package com.sunny.framework.cache;

import org.redisson.misc.RedisURI;
import org.springframework.boot.autoconfigure.data.redis.RedisConnectionDetails;
import org.springframework.boot.autoconfigure.data.redis.RedisProperties;

import java.util.List;
import java.util.stream.Collectors;

public class PropertiesRedisConnectionDetails implements RedisConnectionDetails {

    private final RedisProperties properties;

    PropertiesRedisConnectionDetails(RedisProperties properties) {
        this.properties = properties;
    }

    @Override
    public String getUsername() {
        if (this.properties.getUrl() != null) {
            RedisURI uri = parseURL();
            return uri.getUsername();
        }
        return this.properties.getUsername();
    }

    @Override
    public String getPassword() {
        if (this.properties.getUrl() != null) {
            RedisURI uri = parseURL();
            return uri.getPassword();
        }
        return this.properties.getPassword();
    }

    @Override
    public Standalone getStandalone() {
        if (this.properties.getUrl() != null) {
            RedisURI uri = parseURL();
            return Standalone.of(uri.getHost(), uri.getPort(),
                    this.properties.getDatabase());
        }
        return Standalone.of(this.properties.getHost(), this.properties.getPort(), this.properties.getDatabase());
    }

    private RedisURI parseURL() {
        if (this.properties.getUrl() != null) {
            return new RedisURI(this.properties.getUrl());
        }
        return null;
    }

    @Override
    public Sentinel getSentinel() {
        RedisProperties.Sentinel sentinel = this.properties
                .getSentinel();
        if (sentinel == null) {
            return null;
        }
        return new Sentinel() {

            @Override
            public int getDatabase() {
                return PropertiesRedisConnectionDetails.this.properties.getDatabase();
            }

            @Override
            public String getMaster() {
                return sentinel.getMaster();
            }

            @Override
            public List<Node> getNodes() {
                return sentinel.getNodes().stream().map(PropertiesRedisConnectionDetails.this::asNode).collect(Collectors.toList());
            }

            @Override
            public String getUsername() {
                return sentinel.getUsername();
            }

            @Override
            public String getPassword() {
                return sentinel.getPassword();
            }

        };
    }

    @Override
    public Cluster getCluster() {
        RedisProperties.Cluster cluster = this.properties.getCluster();
        if (cluster != null) {
            return () -> cluster.getNodes().stream()
                                           .map(this::asNode)
                                           .collect(Collectors.toList());
        }
        return null;
    }

    private Node asNode(String node) {
        String[] components = node.split(":");
        return new Node(components[0], Integer.parseInt(components[1]));
    }

}
