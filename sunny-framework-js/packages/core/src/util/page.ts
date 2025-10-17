export async function forEachPage(
    fetchPage: (pageIndex: number, pageSize: number) => Promise<any[]>,
    handleList: (pageIndex: number, pageSize: number, list: any[]) => Promise<void>,
    pageSize = 100
) {
    let pageIndex = 1;
    while (true) {

        const list = await fetchPage(pageIndex, pageSize);

        if (!list || list.length === 0) break;

        await handleList(pageIndex, pageSize, list);

        pageIndex++;
        if (list.length < pageSize) break;
    }
}