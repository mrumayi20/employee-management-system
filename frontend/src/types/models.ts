export type Department = {
  id: string;
  name: string;
  description?: string;
};

export type Employee = {
  id: string;
  employeeCode: string;
  fullName: string;
  email: string;
  phone: string;
  departmentName: string;
};