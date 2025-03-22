using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TodoApp.DataAccess;
using TodoApp.Models;
using TodoApp.Reposotory;
using TodoApp.Reposotory.IRepository;


namespace TodoApp.Areas.User.Controllers
{
    [Area("User")]
    public class TaskController : Controller
    {
        private readonly ITaskItemRepository _taskItemRepository;
        public TaskController(ITaskItemRepository _taskItemRepository)
        {
            this._taskItemRepository = _taskItemRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var data = _taskItemRepository.Get();
            return View(data.ToList());
        }
        [HttpPost]
        public IActionResult SaveTaskData(TaskItem task)
        {
            if (task != null)
            {

                _taskItemRepository.Create(task);
                _taskItemRepository.comit();
            }

            return RedirectToAction("index", "Task");
        }

        [HttpPost]
        public IActionResult Delete(int TaskId)
        {
            var task = _taskItemRepository.GetOne(e => e.Id == TaskId);
            if (task != null)
            {
                _taskItemRepository.Delete(task);
                _taskItemRepository.comit();
            }
            return RedirectToAction("Index", "Task");

        }
        public IActionResult EditTaskView(int TaskId)
        {
            var task = _taskItemRepository.GetOne(e => e.Id == TaskId);

            if (task != null)
            {
                return View(task);
            }
            return RedirectToAction("error", "Home");

        }
        public IActionResult SaveEditedData(TaskItem task)
        {
            if (task != null)
            {
                _taskItemRepository.Edit(task);
                _taskItemRepository.comit();
                return RedirectToAction("index", "task");
            }

            return RedirectToAction("error", "Home");



        }
        [HttpPost]
        public IActionResult MarkCompleted(int TaskID)
        {
            var task = _taskItemRepository.GetOne(e => e.Id == TaskID);
            if (task != null)
            {
                task.Status = !task.Status;
                _taskItemRepository.comit();
                return RedirectToAction("index", "Task");

            }
            return RedirectToAction("error", "Home");


        }
        [HttpGet]
        public IActionResult search(string keyword)
        {
            var data = _taskItemRepository.Get(e => e.Title.Contains(keyword)).ToList();
            return View("Index", data.ToList());


        }
        public IActionResult DownloadPdf(int id)
        {
            var tasks = _taskItemRepository.Get(e => e.Id == id);

            var pdfDocument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(14));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("To-Do List")
                                .SemiBold().FontSize(22).FontColor(Colors.Blue.Medium);
                            col.Item().Text("Generated Tasks Report")
                                .Italic().FontSize(14).FontColor(Colors.Grey.Darken2);
                        });

                        row.ConstantItem(100).Image("wwwroot/logo/logo.png", ImageScaling.FitArea);
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.ConstantColumn(120);
                            columns.ConstantColumn(120);
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(e => e.Border(1).Padding(5).Background(Colors.Blue.Lighten3)).Text("#").Bold();
                            header.Cell().Element(e => e.Border(1).Padding(5).Background(Colors.Blue.Lighten3)).Text("Task Title").Bold();
                            header.Cell().Element(e => e.Border(1).Padding(5).Background(Colors.Blue.Lighten3)).Text("Description").Bold();
                            header.Cell().Element(e => e.Border(1).Padding(5).Background(Colors.Blue.Lighten3)).Text("Created At").Bold();
                            header.Cell().Element(e => e.Border(1).Padding(5).Background(Colors.Blue.Lighten3)).Text("Deadline").Bold();
                            header.Cell().Element(e => e.Border(1).Padding(5).Background(Colors.Blue.Lighten3)).Text("Status").Bold();
                        });

                        int index = 1;
                        foreach (var task in tasks)
                        {
                            table.Cell().Element(e => e.Padding(5)).Text(index++.ToString());
                            table.Cell().Element(e => e.Padding(5)).Text(task.Title ?? "N/A".ToString());
                            table.Cell().Element(e => e.Padding(5)).Text(task.Description ?? "No Description".ToString());
                            table.Cell().Element(e => e.Padding(5)).Text(task.CreatedAt.ToString("dd/MM/yyyy HH:mm"));
                            table.Cell().Element(e => e.Padding(5)).Text(task.Deadline.ToString("dd/MM/yyyy HH:mm"));

                            table.Cell().Element(e => e.Padding(5)).Text(task.Status.HasValue ? task.Status.Value ? "Completed" : "Pending" : "Unknown").FontColor(task.Status.HasValue && task.Status.Value ? Colors.Green.Darken2 : Colors.Red.Darken2);
                        }
                    });


                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated on {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(12)
                        .FontColor(Colors.Grey.Darken2);
                });
            });

            byte[] pdfBytes;
            using (var stream = new MemoryStream())
            {
                pdfDocument.GeneratePdf(stream);
                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", "ToDoList.pdf");
        }
    }
}

