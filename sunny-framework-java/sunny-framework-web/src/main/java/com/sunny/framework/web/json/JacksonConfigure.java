package com.sunny.framework.web.json;

import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.databind.DeserializationContext;
import com.fasterxml.jackson.databind.JsonDeserializer;
import com.fasterxml.jackson.databind.SerializationFeature;
import org.apache.commons.lang3.StringUtils;
import org.springframework.boot.autoconfigure.jackson.Jackson2ObjectMapperBuilderCustomizer;
import org.springframework.context.annotation.Bean;
import org.springframework.core.annotation.Order;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

public class JacksonConfigure {

    public static final String DATE_FORMAT_PATTERN = "yyyy-MM-dd HH:mm:ss";

    /**
     * format to timestamp
     * <pre>
     * @Bean
     * @Order(10)
     * public Jackson2ObjectMapperBuilderCustomizer jackson2ObjectMapperBuilderCustomizer() {
     * return builder -> {
     * builder.serializerByType(LocalDateTime.class, LocalDateTimeSerializer.INSTANCE);
     * builder.deserializerByType(LocalDateTime.class, LocalDateTimeDeserializer.INSTANCE);
     * builder.deserializerByType(String.class, stringStdScalarDeserializer());
     * builder.featuresToEnable(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS);
     * };
     * }
     * </pre>
     */
    @Bean
    @Order(10)
    public Jackson2ObjectMapperBuilderCustomizer portalJacksonCustomConfig() {
        DateTimeFormatter dateTimeFormatter = DateTimeFormatter.ofPattern(DATE_FORMAT_PATTERN);
        return (builder) -> {
            builder.serializerByType(LocalDateTime.class, new LocalDateTimeSerializer(dateTimeFormatter));
            builder.deserializerByType(LocalDateTime.class, new LocalDateTimeDeserializer(dateTimeFormatter));
            builder.dateFormat(new SimpleDateFormat(DATE_FORMAT_PATTERN));
            builder.featuresToDisable(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS);
            builder.deserializerByType(String.class, stringStdScalarDeserializer());
        };
    }

    public JsonDeserializer<String> stringStdScalarDeserializer() {
        return new JsonDeserializer<>() {
            @Override
            public String deserialize(JsonParser p, DeserializationContext ctxt) throws IOException {
                return StringUtils.trim(p.getValueAsString());
            }
        };
    }
}
