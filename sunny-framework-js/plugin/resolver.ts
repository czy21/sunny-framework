import path from 'path'

const resolveFrameworkImporter = (rootDir) => {
    return {
        name: 'resolve-framework-importer',
        async resolveId(source, importer, options) {
            if (/sunny-framework-js/.test(importer)) {
                const resolution = await this.resolve(path.join(rootDir,`node_modules/${source}`), importer, options);
                return resolution.id;
            }
        }
    }
}

export {resolveFrameworkImporter}