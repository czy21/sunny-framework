package com.sunny.framework.core.model;

public class PagingParam {
    public static final int DEFAULT_PAGE = 1;
    public static final int DEFAULT_PAGE_SIZE = 10;
    private int page = DEFAULT_PAGE;
    private int pageSize = DEFAULT_PAGE_SIZE;

    public int getPage() {
        return page;
    }

    public void setPage(int page) {
        this.page = page;
    }

    public int getPageSize() {
        return pageSize;
    }

    public void setPageSize(int pageSize) {
        this.pageSize = pageSize;
    }

    public final int getOffset() {
        return (this.page - 1) * this.pageSize;
    }
}
