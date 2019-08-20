using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/todo")]
    //[Route("api/[controller]")]
    [ApiController] // Этот атрибут указывает, что контроллер отвечает на запросы веб-API
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                // Создать новый TodoItem, если коллекция пуста,
                // это означает, что вы не можете удалить все TodoItems.
                _context.TodoItems.Add(new TodoItem { Name = "Item1" });
                _context.SaveChanges();
            }
        }

        [HttpGet] // GET /api/todo
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: /api/todo/{id}
        // Это переменная-заполнитель для уникального идентификатора элемента задачи.
        // При вызове GetTodoItem параметру метода id присваивается значение "{id}" в URL-адресе.
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            // автоматически сериализует объект в формат JSON и записывает данные JSON в тело сообщения ответа.
            return todoItem;
        }        // Типы возвращаемых значений ActionResult могут представлять широкий спектр кодов состояний HTTP.
                 // Например, метод GetTodoItem может возвращать два разных значения состояния:
                 // Если запрошенному идентификатору не соответствует ни один элемент, метод возвращает ошибку 
                 // 404 (Не найдено).
                 // В противном случае метод возвращает код 200 с телом ответа JSON.При возвращении item возвращается
                 // ответ HTTP 200.


        // Предыдущий код является методом HTTP POST, как указывает атрибут [HttpPost].
        // Этот метод получает значение элемента списка дел из текста HTTP-запроса.
        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem item)
        {
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();

            
            // Ключевое слово nameof C# используется для предотвращения жесткого 
            // программирования имени действия в вызове CreatedAtAction.
            return CreatedAtAction(nameof(GetTodoItem), new { id = item.Id }, item);
        }
        // В случае успеха возвращает код состояния HTTP 201. HTTP 201 представляет
        // собой стандартный ответ для метода HTTP POST, создающий ресурс на сервере.

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // Страница PutTodoItem аналогична странице PostTodoItem, но использует запрос HTTP PUT.
        // Ответ — 204 (Нет содержимого). Согласно спецификации HTTP, запрос PUT требует, чтобы
        // клиент отправлял всю обновленную сущность, а не только изменения. Чтобы обеспечить поддержку
        // частичных обновлений, используйте HTTP PATCH.

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // DeleteTodoItemОтвет — 204 (нет содержимого).
    }
}