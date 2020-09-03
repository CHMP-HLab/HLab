using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Options.Data
{
    using H = HD<Option>;

    public class Option : Entity
    {
        public int? UserId
        {
            get => _userId.Get();
            set => _userId.Set(value);
        }
        private readonly IProperty<int?> _userId = H.Property<int?>();

        [Ignore]
        public User User
        {
            get => _user.Get();
            set => UserId = value.Id;
        }
        private readonly IProperty<User> _user = H.Property<User>(c => c.Foreign(e => e.UserId));

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>();

        public string Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }
        private readonly IProperty<string> _value = H.Property<string>();
    }
}
