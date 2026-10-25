// PreToolUse (Edit|Write): block changes to existing files under api.Tests/.
import fs from "node:fs";
import { readInput, projectRelative, isUnder } from "./lib.mjs";

const input = await readInput();
const target = projectRelative(input);

if (target && isUnder(target.rel, "api.Tests") && fs.existsSync(target.abs)) {
  console.error(
    `Blocked: ${target.rel} is an existing test file and existing tests can't be edited. ` +
      "Stop and tell the user which test you wanted to change and why, instead of working around this."
  );
  process.exit(2);
}
process.exit(0);
