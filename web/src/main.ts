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

form.addEventListener('submit', async (event) => {
  event.preventDefault()
  submit.disabled = true
  try {
    const name = new FormData(form).get('name') as string
    await createBoard(name)
    form.reset()
  } finally {
    submit.disabled = false
  }
})
