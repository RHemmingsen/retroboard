// PostToolUse (Edit|Write): run the web build after changes under web/src/.
import path from "node:path";
import { spawnSync } from "node:child_process";
import { readInput, projectRelative, isUnder } from "./lib.mjs";

const input = await readInput();
const target = projectRelative(input);

if (!target || !isUnder(target.rel, "web/src")) process.exit(0);

const result = spawnSync("npm run build", {
  cwd: path.join(target.root, "web"),
  shell: true,
  encoding: "utf8",
});

if (result.status !== 0) {
  console.error(`npm run build failed in web/ after editing ${target.rel}:\n`);
  console.error(result.stdout + result.stderr);
  process.exit(2);
}
process.exit(0);
