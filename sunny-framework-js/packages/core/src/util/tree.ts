export const override = (t1: any, t2: any, idKey: string = "id", parentKey: string = "parentId") => {

    if (t1[idKey] === t2[idKey]) {
        let mergedNode: any = {...t1, ...t2}
        const childMap = new Map();

        for (const child of t1.children || []) {
            childMap.set(child[idKey], child);
        }

        for (const child of t2.children || []) {
            childMap.set(child[idKey], childMap.has(child[idKey]) ? override(childMap.get(child[idKey]), child) : child);
        }

        mergedNode.children = Array.from(childMap.values());
        return mergedNode;
    }

    return t1.children;
}

export const build = (all: any[], attr: { idKey?: string, parentKey?: string, pathsKey?: string, sortKey?: string } = {}, decoFunc?: (node: any) => void) => {
    attr = {idKey: "id", parentKey: "parentId", pathsKey: "paths", sortKey: "sort", ...attr}
    let root: any = {}
    buildChildren(all, root, attr, decoFunc, 0)
    return root.children
}

export const buildChildren = (all: any[], node: any, attr: { idKey?: string, parentKey?: string, pathsKey?: string, sortKey?: string } = {}, decoFunc: (node: any) => void, level: number) => {

    decoFunc && decoFunc(node)
    node.level = level

    for (var t of all) {
        if (node[attr.idKey] == t[attr.parentKey]) {
            buildChildren(all, t, attr, decoFunc, level + 1);
            node.children = node.children || []
            node.children.push(t)
            node.children.sort((a: any, b: any) => a[attr.sortKey] - b[attr.sortKey])
        }
    }
}

export const buildByPath = (all: any[], attr: { idKey?: string, parentKey?: string, pathsKey?: string, sortKey?: string } = {}, decoFunc?: (item: any, node: any, pathIndex: number) => void) => {
    attr = {idKey: "id", parentKey: "parentId", pathsKey: "paths", sortKey: "sort", ...attr}
    let root: any = {}
    for (var t of all) {
        let node = root
        t[attr.pathsKey]?.forEach((p: any, i: number, a: any[]) => {
            node.children = node.children || []
            let pVal = typeof p == 'object' ? p[attr.idKey] : p
            let child = node.children.find((c: any) => c[attr.idKey] == pVal)
            if (!child) {
                child = {}
                child[attr.idKey] = pVal
                child[attr.parentKey] = node[attr.idKey]
                child = i == a.length - 1 ? {...t, ...child} : child
                node.children.push(child)
            }

            if (typeof p === 'object') {
                Object.entries(p).forEach(([k, v]) => child[k] = v)
            }

            decoFunc && decoFunc(t, node, i)

            node.children.sort((a: any, b: any) => a[attr.sortKey] - b[attr.sortKey])

            node = child
        })
    }
    return root.children
}

export const flatten = (root,childKey='children')=>{
  const result = []
  if (root[childKey]?.length) {
    for (const child of root[childKey]) {
      result.push(child, ...flatten(child, childKey))
    }
  }
  return result
}