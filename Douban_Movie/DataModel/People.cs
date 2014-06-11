using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanoramaApp2
{
    public class People : INotifyPropertyChanged
    {
        private string _id = "";
        public string id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id == value)
                {
                    return;
                }
                _id = value;
                NotifyPropertyChanged("id");
            }
        }
        private string _name = "";
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                {
                    return;
                }
                _name = value;
                NotifyPropertyChanged("name");
            }
        }
        private string _position = "";
        public string position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position == value)
                {
                    return;
                }
                _position = value;
                NotifyPropertyChanged("position");
            }
        }
        private string _posterUrl = "";
        public string posterUrl
        {
            get
            {
                return _posterUrl;
            }
            set
            {
                if (_posterUrl == value)
                {
                    return;
                }
                _posterUrl = value;
                NotifyPropertyChanged("posterUrl");
            }
        }
        private string _positionName = "";
        public string positionName
        {
            get
            {
                return _positionName;
            }
            set
            {
                if (_positionName == value)
                {
                    return;
                }
                _positionName = value;
                NotifyPropertyChanged("positionName");
            }
        }
        private string _gender = "";
        public string gender
        {
            get
            {
                return _gender;
            }
            set
            {
                if (_gender == value)
                {
                    return;
                }
                _gender = value;
                NotifyPropertyChanged("gender");
            }
        }
        private string _constl = "";
        public string constl
        {
            get
            {
                return _constl;
            }
            set
            {
                if (_constl == value)
                {
                    return;
                }
                _constl = value;
                NotifyPropertyChanged("constl");
            }
        }
        private string _birthday = "";
        public string birthday
        {
            get
            {
                return _birthday;
            }
            set
            {
                if (_birthday == value)
                {
                    return;
                }
                _birthday = value;
                NotifyPropertyChanged("birthday");
            }
        }
        private string _birthplace = "";
        public string birthplace
        {
            get
            {
                return _birthplace;
            }
            set
            {
                if (_birthplace == value)
                {
                    return;
                }
                _birthplace = value;
                NotifyPropertyChanged("birthplace");
            }
        }
        private string _occupation = "";
        public string occupation
        {
            get
            {
                return _occupation;
            }
            set
            {
                if (_occupation == value)
                {
                    return;
                }
                _occupation = value;
                NotifyPropertyChanged("birthplace");
            }
        }
        private string _summary = "";
        public string summary
        {
            get
            {
                return _summary;
            }
            set
            {
                if (_summary == value)
                {
                    return;
                }
                _summary = value;
                NotifyPropertyChanged("summary");
            }
        }

        private string _nextMovieLink = "";
        public string nextMovieLink
        {
            get
            {
                return _nextMovieLink;
            }
            set
            {
                _nextMovieLink = value;
            }
        }

        private string _nextImageLink = "";
        public string nextImageLink
        {
            get
            {
                return _nextImageLink;
            }
            set
            {
                _nextImageLink = value;
            }
        }

        public static string DIRECTOR = "director";
        public static string ACTOR = "actor";
        public static string peopleLinkHeader = "http://movie.douban.com/celebrity/";

        public event PropertyChangedEventHandler PropertyChanged;
        // NotifyPropertyChanged will raise the PropertyChanged event, 
        // passing the source property that is being updated.
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is People)
            {
                People tmp = obj as People;
                return tmp.id == this.id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}
