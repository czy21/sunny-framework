package com.sunny.framework.core.model;

import org.apache.commons.lang3.ObjectUtils;

import java.util.List;

public interface TreeNode<T> extends Comparable<TreeNode<T>> {

    T getId();

    default void setId(T id) {
    }

    T getParentId();

    default void setParentId(T parentId) {
    }

    default List<T> getParentIds() {
        return null;
    }

    default void setParentIds(List<T> parentIds) {
    }

    default List<T> getPathIds() {
        return null;
    }

    default void setPathIds(List<T> pathIds) {
    }

    default Integer getLevel() {
        return null;
    }

    default void setLevel(Integer level) {
    }

    List<? extends TreeNode<T>> getChildren();

    default void setChildren(List<? extends TreeNode<T>> children) {
    }

    default Integer getSort() {
        return 0;
    }

    default void setSort(Integer sort) {
    }

    @Override
    default int compareTo(TreeNode<T> o) {
        return ObjectUtils.compare(this.getSort(), o.getSort());
    }
}
