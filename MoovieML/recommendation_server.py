from flask import Flask, request, jsonify
import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import linear_kernel

app = Flask(__name__)

@app.route('/recommend', methods=['POST'])
def recommend():
    try:
        data = request.get_json()
        
        liked_movies = data.get('liked_movies', [])
        candidate_movies = data.get('candidate_movies', [])

        if not liked_movies or not candidate_movies:
            return jsonify({'recommendations': []})

        liked_df = pd.DataFrame(liked_movies)
        liked_df['type'] = 'liked'
        
        candidate_df = pd.DataFrame(candidate_movies)
        candidate_df['type'] = 'candidate'
        
        liked_ids = set(liked_df['id'])
        candidate_df = candidate_df[~candidate_df['id'].isin(liked_ids)]

        if candidate_df.empty:
            return jsonify({'recommendations': []})

        full_df = pd.concat([liked_df, candidate_df], ignore_index=True)

        full_df['content'] = full_df['genres'].fillna('') + " " + full_df['overview'].fillna('')

        tfidf = TfidfVectorizer(stop_words='english')
        tfidf_matrix = tfidf.fit_transform(full_df['content'])

        cosine_sim = linear_kernel(tfidf_matrix, tfidf_matrix)

        liked_indices = full_df[full_df['type'] == 'liked'].index
        candidate_indices = full_df[full_df['type'] == 'candidate'].index

        results = []
        for cand_idx in candidate_indices:
            sim_scores = cosine_sim[cand_idx, liked_indices]
            avg_score = sim_scores.mean() if len(sim_scores) > 0 else 0
            results.append((int(full_df.iloc[cand_idx]['id']), avg_score))

        results = sorted(results, key=lambda x: x[1], reverse=True)
        recommended_ids = [r[0] for r in results[:10]]

        return jsonify({'recommendations': recommended_ids})

    except Exception as e:
        print(f"Error: {e}")
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)