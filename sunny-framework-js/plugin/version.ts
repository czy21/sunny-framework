import fs from "fs";
import path from "path";

export const generateVersion = (rootDir: string, outDir = "build", versionValue?: {} = {}) => {
    return {
        name: 'generate-version',
        closeBundle() {
            fs.writeFile(path.join(rootDir, outDir, "version.json"), JSON.stringify({"buildDate": new Date().getTime(), ...versionValue}), (err) => null)
        }
    }
}