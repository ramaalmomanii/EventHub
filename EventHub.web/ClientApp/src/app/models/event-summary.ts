export interface EventSummary {
  eventId: number;
  summary: string;
  provider: string;
  generatedAt: string;
}

export type AiProvider = 'openai' | 'gemini';
