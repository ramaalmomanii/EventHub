import { Component } from '@angular/core';

interface Event {
  title: string;
  date: string;
  location: string;
}

@Component({
  selector: 'app-event-list',
  standalone: true, 
  imports: [],  
  templateUrl: './event-list.html',
  styleUrl: './event-list.scss'
})
export class EventList {
  events: Event[] = [
    { title: 'Concert 2026', date: '2026-06-15', location: 'Amman' },
    { title: 'Tech Conference', date: '2026-07-20', location: 'Irbid' }
  ];
}
