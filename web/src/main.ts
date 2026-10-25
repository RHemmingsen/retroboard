import './style.css'

type Board = { id: string; name: string; createdUtc: string }
type Card = { id: string; boardId: string; text: string }

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
<section>
  <h2>Cards</h2>
  <form id="load-cards">
    <label for="cards-board-id">Board id</label>
    <input id="cards-board-id" name="boardId" required />
    <button type="submit">Load</button>
  </form>
  <p id="cards-error" role="alert"></p>
  <section id="cards" hidden>
    <ul id="card-list"></ul>
    <p id="cards-empty" hidden>No cards yet.</p>
    <form id="create-card">
      <label for="card-text">Text</label>
      <textarea id="card-text" name="text" required maxlength="500"></textarea>
      <button type="submit">Add card</button>
    </form>
  </section>
</section>
`

async function readError(response: Response): Promise<Error> {
  if (response.status === 404) {
    return new Error('Board not found')
  }
  const problem = await response.json().catch(() => null)
  return new Error(problem?.detail ?? problem?.title ?? response.statusText)
}

async function createBoard(name: string): Promise<Board> {
  const response = await fetch('/api/boards', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name }),
  })

  if (!response.ok) {
    throw await readError(response)
  }

  return response.json()
}

async function listCards(boardId: string): Promise<Card[]> {
  const response = await fetch(`/api/boards/${encodeURIComponent(boardId)}/cards`)

  if (!response.ok) {
    throw await readError(response)
  }

  return response.json()
}

async function createCard(boardId: string, text: string): Promise<Card> {
  const response = await fetch(`/api/boards/${encodeURIComponent(boardId)}/cards`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ text }),
  })

  if (!response.ok) {
    throw await readError(response)
  }

  return response.json()
}

const form = document.querySelector<HTMLFormElement>('#create-board')!
const submit = form.querySelector<HTMLButtonElement>('button[type="submit"]')!
const error = document.querySelector<HTMLParagraphElement>('#error')!
const result = document.querySelector<HTMLElement>('#result')!
const resultName = document.querySelector<HTMLSpanElement>('#result-name')!
const resultId = document.querySelector<HTMLElement>('#result-id')!

const loadForm = document.querySelector<HTMLFormElement>('#load-cards')!
const loadSubmit = loadForm.querySelector<HTMLButtonElement>('button[type="submit"]')!
const boardIdInput = document.querySelector<HTMLInputElement>('#cards-board-id')!
const cardsError = document.querySelector<HTMLParagraphElement>('#cards-error')!
const cards = document.querySelector<HTMLElement>('#cards')!
const cardList = document.querySelector<HTMLUListElement>('#card-list')!
const cardsEmpty = document.querySelector<HTMLParagraphElement>('#cards-empty')!
const cardForm = document.querySelector<HTMLFormElement>('#create-card')!
const cardSubmit = cardForm.querySelector<HTMLButtonElement>('button[type="submit"]')!

// The board whose cards are shown; the input may be edited without reloading.
let loadedBoardId: string | null = null

function appendCard(card: Card) {
  const item = document.createElement('li')
  item.textContent = card.text
  cardList.append(item)
  cardsEmpty.hidden = true
}

async function loadCards(boardId: string) {
  loadSubmit.disabled = true
  try {
    const list = await listCards(boardId)
    loadedBoardId = boardId
    cardList.replaceChildren()
    list.forEach(appendCard)
    cardsEmpty.hidden = list.length > 0
    cards.hidden = false
    cardsError.textContent = ''
  } catch (e) {
    loadedBoardId = null
    cards.hidden = true
    cardsError.textContent = e instanceof Error ? e.message : String(e)
  } finally {
    loadSubmit.disabled = false
  }
}

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
    boardIdInput.value = board.id
    await loadCards(board.id)
  } catch (e) {
    error.textContent = e instanceof Error ? e.message : String(e)
  } finally {
    submit.disabled = false
  }
})

loadForm.addEventListener('submit', async (event) => {
  event.preventDefault()
  await loadCards(boardIdInput.value.trim())
})

cardForm.addEventListener('submit', async (event) => {
  event.preventDefault()
  if (loadedBoardId === null) return
  cardSubmit.disabled = true
  try {
    const text = new FormData(cardForm).get('text') as string
    appendCard(await createCard(loadedBoardId, text))
    cardsError.textContent = ''
    cardForm.reset()
  } catch (e) {
    cardsError.textContent = e instanceof Error ? e.message : String(e)
  } finally {
    cardSubmit.disabled = false
  }
})
