using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HLab.Notify.PropertyChanged;

using H = HLab.Notify.PropertyChanged.NotifyHelper<HLab.Acl.User>;

namespace HLab.Acl
{
    public class User : INotifyPropertyChanged
    {
        public User()
        {
            H.Initialize(this, args => PropertyChanged?.Invoke(this, args));
        }

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));

        public string FirstName
        {
            get => _firstName.Get();
            set => _firstName.Set(value);
        }
        private readonly IProperty<string> _firstName = H.Property<string>(c => c.Default(""));

        public string Initials
        {
            get => _initials.Get();
            set => _initials.Set(value);
        }
        private readonly IProperty<string> _initials = H.Property<string>(c => c.Default(""));

        public string Login
        {
            get => _login.Get();
            set => _login.Set(value);
        }
        private readonly IProperty<string> _login = H.Property<string>(c => c.Default(""));

        public string HashedPassword
        {
            get => _hashedPassword.Get();
            set => _hashedPassword.Set(value);
        }
        private readonly IProperty<string> _hashedPassword = H.Property<string>(c => c.Default(""));

        public int? RightId
        {
            get => _rightId.Get();
            set => _rightId.Set(value);
        }
        private readonly IProperty<int?> _rightId = H.Property<int?>();

         public string Function
        {
            get => _function.Get();
            set => _function.Set(value);
        }
        private readonly IProperty<string> _function = H.Property<string>(c => c.Default(""));

        public String Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }
        private readonly IProperty<string> _note = H.Property<string>(c => c.Default(""));

        public string Pin
        {
            get => _pin.Get();
            set => _pin.Set(value);
        }
        private readonly IProperty<string> _pin = H.Property<string>(c => c.Default(""));

        public DateTime? Expiry
        {
            get => _expiry.Get();
            set => _expiry.Set(value);
        }
        private readonly IProperty<DateTime?> _expiry = H.Property<DateTime?>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
