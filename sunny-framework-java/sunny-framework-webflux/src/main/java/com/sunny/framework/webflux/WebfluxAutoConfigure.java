package com.sunny.framework.webflux;


import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.PropertySource;
import org.springframework.web.reactive.config.EnableWebFlux;
import org.springframework.web.reactive.config.WebFluxConfigurer;

@PropertySource("classpath:sunny-common-webflux-default.properties")
@EnableWebFlux
@Configuration
public class WebfluxAutoConfigure implements WebFluxConfigurer {

}
