export interface User {
    id: number,
    userName: string,
    firstName: string,
    lastName: string,
    status: number,
    email: string,
    lastLogin: string,
    dateOfBirth: {
        year: number,
        month: number,
        day: number,
        dayOfWeek: number,
        dayOfYear: number,
        dayNumber: number
    },
    createdAt: string,
    updatedAt: string,
    avatar: string,
    onlinestatus: string,
    token: string
}