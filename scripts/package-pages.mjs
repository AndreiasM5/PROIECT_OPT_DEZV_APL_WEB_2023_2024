import { cp, readdir, mkdir, rm, writeFile } from 'node:fs/promises';
import { fileURLToPath } from 'node:url';
import path from 'node:path';

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const source = path.join(root, 'Frontend/dist/store/browser');
const target = path.join(root, 'docs');
await mkdir(target, {recursive: true});
// Remove only previous generated bundles. Preserve the README screenshot.
for (const entry of await readdir(target, {withFileTypes: true})) {
  if (entry.isFile() && /^(main|styles|chunk)-[a-zA-Z0-9_-]+\.(js|css|js\.map|css\.map)$/.test(entry.name)) {
    await rm(path.join(target, entry.name));
  }
}
await cp(source, target, {recursive: true});
await writeFile(path.join(target, '.nojekyll'), '');
console.log('GitHub Pages build prepared in docs/ (base path: /PROIECT_OPT_DEZV_APL_WEB_2023_2024/).');
