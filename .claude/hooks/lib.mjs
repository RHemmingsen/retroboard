import path from "node:path";

export async function readInput() {
  let raw = "";
  for await (const chunk of process.stdin) raw += chunk;
  return JSON.parse(raw);
}

// Returns the file path relative to the project root using forward slashes,
// or null if the file is outside the project.
export function projectRelative(input) {
  const filePath = input.tool_input?.file_path;
  if (!filePath) return null;
  const root = process.env.CLAUDE_PROJECT_DIR || input.cwd || process.cwd();
  const abs = path.resolve(input.cwd || root, filePath);
  let rel = path.relative(root, abs);
  // Windows paths are case-insensitive; drive letters often differ in case.
  if (process.platform === "win32" && path.isAbsolute(rel)) {
    rel = path.relative(root.toLowerCase(), abs.toLowerCase());
  }
  if (rel.startsWith("..") || path.isAbsolute(rel)) return null;
  return { root, abs, rel: rel.split(path.sep).join("/") };
}

export function isUnder(rel, dir) {
  const a = process.platform === "win32" ? rel.toLowerCase() : rel;
  const b = process.platform === "win32" ? dir.toLowerCase() : dir;
  return a.startsWith(b.endsWith("/") ? b : b + "/");
}
