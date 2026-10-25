import './style.css'

type Board = { id: string; name: string; createdUtc: string }

document.querySelector<HTMLDivElement>('#app')!.innerHTML = `
<section>
  <h1>Create a board</h1>
  <form id="create-board">
    <label for="board-name">Name</label>
    <input id="board-name" name="name" required maxlength="80" />
    <button type="submit">Create</button>
  </form>
  <p id="error" role="alert"></p>
  <section id="result" hidden>
    <h2>Created board</h2>
    <p>Name: <span id="result-name"></span></p>
    <p>Id: <code id="result-id"></code></p>
  </section>
</section>
`

async function createBoard(name: string): Promise<Board> {
  const response = await fetch('/api/boards', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name }),
  })

  if (!response.ok) {
    const problem = await response.json().catch(() => null)
    throw new Error(problem?.detail ?? problem?.title ?? response.statusText)
  }

  return response.json()
}

const form = document.querySelector<HTMLFormElement>('#create-board')!
const submit = form.querySelector<HTMLButtonElement>('button[type="submit"]')!
const error = document.querySelector<HTMLParagraphElement>('#error')!
const result = document.querySelector<HTMLElement>('#result')!
const resultName = document.querySelector<HTMLSpanElement>('#result-name')!
const resultId = document.querySelector<HTMLElement>('#result-id')!

form.addEventListener('submit', async (event) => {
  event.preventDefault()
  submit.disabled = true
  try {
    const name = new FormData(form).get('name') as string
    const board = await createBoard(name)
    resultName.textContent = board.name
    resultId.textContent = board.id
    result.hidden = false
    error.textContent = ''
    form.reset()
  } catch (e) {
    error.textContent = e instanceof Error ? e.message : String(e)
  } finally {
    submit.disabled = false
  }
})
