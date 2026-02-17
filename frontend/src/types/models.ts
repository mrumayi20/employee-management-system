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


export type Attendance = {
  id: string;
  employeeId: string;
  employeeName: string;
  employeeCode: string;
  departmentName: string;
  date: string;       // "YYYY-MM-DD"
  status: string;     // "Present" etc
  checkIn?: string | null;   // "09:30:00"
  checkOut?: string | null;
};