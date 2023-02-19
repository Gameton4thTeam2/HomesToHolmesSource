using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace HTH
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : property 를 위한 data binder 및 attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BindPropertyToAttribute : Attribute
    {
        public string PropertyName
        {
            get;
            private set;
        }

        public SourceTag tag
        {
            get;
            private set;
        }

        public BindPropertyToAttribute(string propertyName, SourceTag sourceId)
        {
            PropertyName = propertyName;
            tag = sourceId;
        }
    }

    /// <summary>
    /// 프로퍼티를 소스에 바인딩해줌.
    /// </summary>
    /// <typeparam name="T">의존시킬 프로퍼티를 멤버로 가지는 객체타입 (Recevier의 타입)</typeparam>
    public class PropertyBinder<T>
    {
        private readonly T _receiver;
        private readonly PropertyDescriptorCollection _sourceProperties;
        private readonly Dictionary<string, PropertyDescriptor> _receiverMappingProperties;
                
        /// <param name="receiver"> 알림을 받을 객체</param>
        /// <param name="source"> 의존시킬 소스. 프로퍼티 값 변화시 receiver 에게 통지함 </param>
        /// <param name="tag"> 소스 구분용 태그 </param>
        public PropertyBinder(T receiver, INotifyPropertyChanged source, SourceTag tag)
        {
            _receiver = receiver;

            // Receiver 의 모든 프로퍼티
            PropertyDescriptorCollection receiverProperties = TypeDescriptor.GetProperties(receiver);

            // Source 의 모든 프로퍼티
            _sourceProperties = TypeDescriptor.GetProperties(source);

            // Receiver 의 모든 프로퍼티중 Binding Attribute 가 붙은 프로퍼티들을 Source 의 프로퍼티와 Mapping 해놓기 위한 사전
            _receiverMappingProperties = new Dictionary<string, PropertyDescriptor>();

            // Source 의 프로퍼티 변화시 통지할 알림 추가
            source.PropertyChanged += SourcePropertyChanged;

            // Attribute 가 있고 tag가 일치하는 프로퍼티들을 Mapping.
            foreach (PropertyDescriptor property in receiverProperties)
            {
                var attribute = (BindPropertyToAttribute)property.Attributes[typeof(BindPropertyToAttribute)];
                if (attribute != null && attribute.tag == tag)
                {
                    _receiverMappingProperties[attribute.PropertyName] = property;
                }
            }
        }

        void SourcePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_receiverMappingProperties.ContainsKey(args.PropertyName))
            {
                _receiverMappingProperties[args.PropertyName]
                    .SetValue(_receiver, _sourceProperties[args.PropertyName]
                    .GetValue(sender));
            }
        }
    }

    public enum SourceTag
    {
        Gold
    }
}