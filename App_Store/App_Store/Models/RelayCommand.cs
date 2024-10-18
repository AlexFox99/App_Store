using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace App_Store.Models
{
    /// Реализация интерфейса ICommand для поддержки команд в MVVM.
    public class RelayCommand : ICommand
    {
        // Делегат для выполнения команды
        private readonly Action<object> _execute;
        // Делегат для проверки возможности выполнения команды
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Конструктор для инициализации RelayCommand.
        /// </summary>
        /// <param name="execute">Метод, который будет выполнен при вызове команды.</param>
        /// <param name="canExecute"></param>
        /// <exception cref="ArgumentNullException">Метод, определяющий, может ли команда быть выполнена.
        /// </exception>

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            // Проверка на null для execute и установка значений
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// Событие, которое вызывается, когда изменяется возможность выполнения команды.
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Определяет, может ли команда быть выполнена.
        /// </summary>
        /// <param name="parameter">Параметр, передаваемый в метод проверки.</param>
        /// <returns>True, если команда может быть выполнена; иначе - false.</returns>
        public bool CanExecute(object parameter)
        {
            // Если _canExecute не задан, команда всегда может быть выполнена
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Выполняет команду.
        /// </summary>
        /// <param name="parameter">Параметр, передаваемый в метод выполнения.</param>
        public void Execute(object parameter)
        {
            // Выполнение действия, связанного с командой
            _execute(parameter);
        }

        /// <summary>
        /// Вызывает событие CanExecuteChanged для обновления состояния команды.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            // Уведомление о том, что возможность выполнения команды изменилась
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
