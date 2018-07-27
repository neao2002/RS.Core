namespace RS.Cache
{
	/// <summary>
	/// ��̭�㷨�Ľӿ�
	/// </summary>
	public interface IEliminateMethod
	{
		/// <summary>
		/// ɾ����СȨֵ
		/// </summary>
		void DelPoorWeight();
		/// <summary>
		/// ���Ȩֵ��С���ID
		/// </summary>
		/// <returns></returns>
		object GetPoorWeightID();
		/// <summary>
		/// ���һ��Ȩֵ��¼
		/// </summary>
		void InsertWeight(object id);
		/// <summary>
		/// ɾ��ĳ��Ȩֵ
		/// </summary>
		void DelWeight(object id);
		/// <summary>
		/// �޸�Ȩֵ
		/// </summary>
		void UpdateWeight(object id);
	}
}
